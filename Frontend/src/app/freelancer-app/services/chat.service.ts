import { Injectable } from '@angular/core';

export interface ChatMessage {
  id: string;
  senderId: string;
  text: string;
  timestamp: string;
}

export interface Chat {
  id: string;
  projectId: string;
  messages: ChatMessage[];
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: HubConnection;
  private messageSubject = new Subject<ChatMessage>();
  private chatSubject = new Subject<Chat>();
  private messageDeletedSubject = new Subject<string>();

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${PROJECTS_SERVICE_API_URL}/chatHub`, { // Adjust URL as needed
        accessTokenFactory: () => this.authService.getAccessToken() || ''
      })
      .build();

    this.setupSignalRListeners();
    this.startConnection();
  }

  private setupSignalRListeners(): void {
    this.hubConnection.on('ReceiveTextMessage', (text: string) => {
      const message: ChatMessage = {
        id: new Date().toISOString(), // Temporary ID, adjust based on backend response
        senderId: '', // Will be filled by backend
        text,
        timestamp: new Date().toISOString()
      };
      this.messageSubject.next(message);
    });

    this.hubConnection.on('ReceiveChat', (chat: Chat) => {
      this.chatSubject.next(chat);
    });

    this.hubConnection.on('MessageIsDeleted', (messageId: string) => {
      this.messageDeletedSubject.next(messageId);
    });

    this.hubConnection.on('ReceiveFileMessage', (fileId: string) => {
      // Handle file message if needed
    });
  }

  private async startConnection(): Promise<void> {
    try {
      await this.hubConnection.start();
      console.log('SignalR Connected');
    } catch (err) {
      console.error('Error starting SignalR connection:', err);
    }
  }

  getMessages(): Observable<ChatMessage> {
    return this.messageSubject.asObservable();
  }

  getChat(): Observable<Chat> {
    return this.chatSubject.asObservable();
  }

  getDeletedMessage(): Observable<string> {
    return this.messageDeletedSubject.asObservable();
  }

  async createChat(projectId: string): Promise<void> {
    const request = { projectId };
    return this.hubConnection.invoke('CreateChat', request);
  }

  async getChatByProjectId(projectId: string): Promise<void> {
    return this.hubConnection.invoke('GetChatByProjectId', projectId);
  }

  async sendMessage(receiverId: string, text: string): Promise<void> {
    const request = { receiverId, text };
    return this.hubConnection.invoke('SendTextMessage', request);
  }

  async getChatMessages(chatId: string): Promise<void> {
    const request = { chatId };
    return this.hubConnection.invoke('GetChatMessages', request);
  }

  async deleteMessage(messageId: string, receiverId: string): Promise<void> {
    const request = { messageId, receiverId };
    return this.hubConnection.invoke('DeleteMessage', request);
  }

  uploadFile(receiverId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('receiverId', receiverId);
    formData.append('file', file);
    return this.http.post(`${PROJECTS_SERVICE_API_URL}/api/files`, formData);
  }
}