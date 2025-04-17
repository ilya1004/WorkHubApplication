import {AfterViewChecked, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzListModule} from "ng-zorro-antd/list";
import {NzTagModule} from "ng-zorro-antd/tag";
import {FormsModule} from "@angular/forms";
import {Message, MessageType} from "../../../../core/interfaces/chat/message.interface";
import {ChatService} from "../../../../core/services/chat/chat.service";
import {AuthService} from "../../../../core/services/auth/auth.service";
import {TokenService} from "../../../../core/services/auth/token.service";
import {Subscription} from "rxjs";
import {TagColor} from "../../../../core/data/tag-color";

@Component({
  selector: 'app-employer-project-chat',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzInputModule,
    NzButtonModule,
    NzListModule,
    NzTagModule,
    FormsModule,
    NzFlexDirective
  ],
  templateUrl: './project-chat.component.html',
  styleUrls: ['./project-chat.component.scss']
})
export class ProjectChatComponent implements OnInit, AfterViewChecked, OnDestroy {
  @Input() projectId!: string;
  @Input() employerId!: string;
  @Input() freelancerId?: string;
  
  chatId: string | null = null;
  messages: Message[] = [];
  newMessage: string = '';
  selectedFile: File | null = null;
  loading = false;
  
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;
  @ViewChild('messageListContainer', { static: false }) messageListContainer!: ElementRef<HTMLDivElement>;
  
  private shouldScroll = false;
  private subscriptions: Subscription[] = [];
  
  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private tokenService: TokenService,
  ) {}
  
  async ngOnInit(): Promise<void> {
    await this.chatService.startConnection();
    await this.initializeChat();
    
    this.subscriptions.push(
      this.chatService.getChatReceived().subscribe(chat => {
        if (chat) {
          this.chatId = chat.id;
          this.loadMessages();
        } else {
          this.createAndFetchChat();
        }
      }),
      this.chatService.getMessageReceived().subscribe(message => {
        if (message.chatId === this.chatId) {
          this.messages.push(message);
          this.shouldScroll = true;
        }
      }),
      this.chatService.getMessagesReceived().subscribe(messages => {
        this.messages = messages.items.reverse();
        this.loading = false;
        this.shouldScroll = true;
      })
    );
  }
  
  ngAfterViewChecked(): void {
    if (this.shouldScroll && this.messageListContainer) {
      this.scrollToBottom();
      this.shouldScroll = false;
    }
  }
  
  ngOnDestroy(): void {
    this.chatService.stopConnection();
    this.messages = [];
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
  
  private scrollToBottom(): void {
    if (this.messageListContainer) {
      const listElement = this.messageListContainer.nativeElement;
      listElement.scrollTop = listElement.scrollHeight;
    }
  }
  
  async initializeChat(): Promise<void> {
    this.loading = true;
    try {
      await this.chatService.getChatByProjectId(this.projectId);
    } catch (error) {
      console.error('Chat initialization error:', error);
    } finally {
      this.loading = false;
    }
  }
  
  private async createAndFetchChat(): Promise<void> {
    try {
      const freelancerId = this.tokenService.getUserId() || '';
      await this.chatService.createChat(this.employerId, freelancerId, this.projectId);
    } catch (error) {
      console.error('Error creating chat:', error);
    }
  }
  
  getMessageSender(message: Message): string {
    const currentUserId = this.tokenService.getUserId();
    if (message.senderId === currentUserId) {
      return 'You';
    }
    return message.senderId === this.employerId ? 'Employer' : 'Freelancer';
  }
  
  private loadMessages(): void {
    if (this.chatId) {
      this.loading = true;
      this.chatService.getChatMessages(this.chatId, 1, 200);
    }
  }
  
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }
  
  sendMessage(): void {
    if (!this.chatId) return;
    
    if (this.newMessage.trim() && !this.selectedFile) {
      this.chatService.sendTextMessage(this.chatId, this.freelancerId!, this.newMessage)
        .then(() => {
          this.newMessage = '';
          this.shouldScroll = true;
        })
        .catch(err => console.error('Error sending message:', err));
    } else if (this.selectedFile && !this.newMessage.trim()) {
      this.chatService.uploadFile(this.chatId, this.freelancerId!, this.selectedFile)
        .subscribe({
          next: () => {
            this.selectedFile = null;
            if (this.fileInput) {
              this.fileInput.nativeElement.value = '';
            }
            this.shouldScroll = true;
          },
          error: (err) => console.error('Error uploading file:', err)
        });
    }
  }
  
  getMessageTypeLabel(): string {
    if (this.newMessage.trim() && !this.selectedFile) {
      return 'Text';
    } else if (this.selectedFile && !this.newMessage.trim()) {
      return 'File';
    } else {
      return 'None';
    }
  }
  
  isTextInputDisabled(): boolean {
    return !!this.selectedFile;
  }
  
  isFileInputDisabled(): boolean {
    return !!this.newMessage.trim();
  }
  
  downloadFile(message: Message): void {
    if (message.type !== MessageType.File || !message.chatId || !message.fileId) {
      console.error('Invalid message for file download:', message);
      return;
    }
    
    this.chatService.downloadFile(message.chatId, message.fileId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = message.fileId!;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.error('Error downloading file:', err);
        if (err.status === 401) {
          console.warn('Unauthorized, token might be expired or invalid');
          this.authService.logout();
        }
      }
    });
  }
  
  protected readonly TagColor = TagColor;
}