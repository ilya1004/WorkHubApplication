import {Component, ElementRef, Input, OnInit, ViewChild} from '@angular/core';
import {NzCardModule} from "ng-zorro-antd/card";
import {NzListModule} from "ng-zorro-antd/list";
import {CommonModule} from "@angular/common";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzButtonModule} from "ng-zorro-antd/button";
import {FormsModule} from "@angular/forms";
import {Message, MessageType} from '../../../interfaces/chat/message.interface';
import {ChatService} from "../../../services/chat.service";
import {AuthService} from "../../../../core/services/auth/auth.service";
import {NzTagModule} from "ng-zorro-antd/tag";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {CHAT_SERVICE_API_URL} from "../../../../core/constants";
import {TokenService} from "../../../../core/services/token/token.service";

@Component({
  selector: 'app-project-chat',
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
export class ProjectChatComponent implements OnInit {
  @Input() projectId!: string;
  @Input() employerId!: string;

  chatId: string | null = null;
  messages: Message[] = [];
  newMessage: string = '';
  selectedFile: File | null = null;
  loading = false;

  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>; // Reference to file input

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private tokenService: TokenService,
  ) {}

  async ngOnInit(): Promise<void> {
    await this.initializeChat();

    this.chatService.getChatReceived().subscribe(chat => {
      if (chat) {
        this.chatId = chat.id;
        this.loadMessages();
      } else {
        console.log('No chat found, creating one');
        this.createAndFetchChat();
      }
    });

    this.chatService.getMessageReceived().subscribe(message => {
      if (message.chatId === this.chatId) {
        this.messages.push(message);
      }
    });

    this.chatService.getMessagesReceived().subscribe(messages => {
      this.messages = messages.items.reverse();
      this.loading = false;
    });
  }

  async initializeChat(): Promise<void> {
    this.loading = true;
    // console.log('Initializing chat for project:', this.projectId);
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
      console.log('Creating new chat with employer:', this.employerId, 'freelancer:', freelancerId);
      await this.chatService.createChat(this.employerId, freelancerId, this.projectId);
    } catch (error) {
      console.error('Error creating chat:', error);
    }
  }

  getMessageSender(message: Message): string {
    return message.senderId === this.tokenService.getUserId() ? 'You' : 'Employer';
  }

  private loadMessages(): void {
    if (this.chatId) {
      this.loading = true;
      this.chatService.getChatMessages(this.chatId, 1, 50);
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
      this.chatService.sendTextMessage(this.chatId, this.employerId, this.newMessage)
        .then(() => {
          this.newMessage = '';
        })
        .catch(err => console.error('Error sending message:', err));
    } else if (this.selectedFile && !this.newMessage.trim()) {
      this.chatService.uploadFile(this.chatId, this.employerId, this.selectedFile)
        .subscribe({
          next: () => {
            console.log('File uploaded');
            this.selectedFile = null;
            if (this.fileInput) {
              this.fileInput.nativeElement.value = ''; // Reset file input
            }
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

  getDownloadUrl(message: Message): string {
    if (message.type === MessageType.File && message.chatId && message.fileId) {
      return `${CHAT_SERVICE_API_URL}files/chat/${message.chatId}/file/${message.fileId}`;
    }
    return '';
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
}