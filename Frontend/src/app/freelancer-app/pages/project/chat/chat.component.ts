import { Component } from '@angular/core';

@Component({
  selector: 'app-chat',
  imports: [],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent implements OnInit {
  @Input() chatId!: string;
  @Input() receiverId!: string;

  messages: { text?: string; fileUrl?: string; isMine: boolean }[] = [];
  form!: FormGroup;
  connection!: HubConnection;
  fileToUpload: File | null = null;

  constructor(
      private fb: FormBuilder,
      private http: HttpClient,
      private message: NzMessageService
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      text: ['']
    });

    this.connection = new HubConnectionBuilder()
        .withUrl('/hubs/chat') // Путь к SignalR
        .withAutomaticReconnect()
        .build();

    this.registerHandlers();

    this.connection.start().then(() => {
      this.getMessages();
    }).catch(err => console.error(err));
  }

  registerHandlers() {
    this.connection.on('ReceiveTextMessage', (message: string) => {
      this.messages.push({ text: message, isMine: false });
    });

    this.connection.on('ReceiveFileMessage', (fileId: string) => {
      const fileUrl = `/api/files/${fileId}`; // предполагаем, что по id можно получить файл
      this.messages.push({ fileUrl, isMine: false });
    });

    this.connection.on('ReceiveChatMessages', (result: any) => {
      const reversed = result.items.reverse();
      this.messages = reversed.map((m: any) => ({
        text: m.text,
        fileUrl: m.fileUrl,
        isMine: m.isMine
      }));
    });
  }

  getMessages() {
    this.connection.invoke('GetChatMessages', {
      chatId: this.chatId,
      pageNo: 1,
      pageSize: 50
    });
  }

  sendMessage() {
    const text = this.form.value.text?.trim();
    if (!text) return;

    this.connection.invoke('SendTextMessage', {
      chatId: this.chatId,
      receiverId: this.receiverId,
      text
    });

    this.messages.push({ text, isMine: true });
    this.form.reset();
  }

  handleFileInput(event: Event) {
    const target = event.target as HTMLInputElement;
    if (target.files && target.files.length > 0) {
      this.fileToUpload = target.files[0];
    }
  }

  uploadFile() {
    if (!this.fileToUpload) return;

    const formData = new FormData();
    formData.append('ChatId', this.chatId);
    formData.append('ReceiverId', this.receiverId);
    formData.append('File', this.fileToUpload);

    this.http.post('/api/files', formData).subscribe({
      next: () => {
        this.messages.push({ fileUrl: 'Файл отправлен', isMine: true });
        this.fileToUpload = null;
      },
      error: () => this.message.error('Ошибка при отправке файла')
    });
  }
}