import { Injectable } from '@angular/core';
import {BehaviorSubject, catchError, Observable, Subject, throwError} from "rxjs";
import {HubConnection, HubConnectionBuilder, HubConnectionState} from "@microsoft/signalr";
import { Chat } from '../../interfaces/chat/chat.interface';
import {Message} from "../../interfaces/chat/message.interface";
import {HttpClient} from "@angular/common/http";
import {AuthService} from "../auth/auth.service";
import {CHAT_SERVICE_API_URL, CHAT_SERVICE_HUB_URL} from "../../data/constants";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";
import {TokenService} from "../token/token.service";

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: HubConnection;
  private messageReceived = new Subject<Message>();
  private chatReceived = new BehaviorSubject<Chat | null>(null);
  private messagesReceived = new Subject<PaginatedResult<Message>>();
  private connectionEstablished = new Subject<boolean>();
  private isHubInitialized = false; // Флаг для предотвращения повторной инициализации
  
  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private tokenService: TokenService
  ) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(CHAT_SERVICE_HUB_URL, {
        accessTokenFactory: () => this.tokenService.getAccessToken() || '',
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build();
    
    // Инициализация обработчиков только один раз при создании сервиса
    this.initializeHub();
  }
  
  private initializeHub(): void {
    if (this.isHubInitialized) {
      return; // Предотвращаем повторную регистрацию обработчиков
    }
    
    this.hubConnection.on('ReceiveChat', (chat: Chat | null) => {
      console.log('Received chat:', chat);
      this.chatReceived.next(chat);
    });
    
    this.hubConnection.on('ReceiveTextMessage', (message: Message) => {
      console.log('Received text message:', message);
      this.messageReceived.next(message);
    });
    
    this.hubConnection.on('ReceiveFileMessage', (message: Message) => {
      console.log('Received file message:', message);
      this.messageReceived.next(message);
    });
    
    this.hubConnection.on('ReceiveChatMessages', (messages: PaginatedResult<Message>) => {
      console.log('Received chat messages:', messages);
      this.messagesReceived.next(messages);
    });
    
    this.hubConnection.onclose((err) => {
      console.error('SignalR connection closed:', err);
    });
    
    this.isHubInitialized = true; // Устанавливаем флаг после инициализации
  }
  
  async startConnection(): Promise<void> {
    if (this.hubConnection.state === HubConnectionState.Disconnected) {
      try {
        console.log('Starting SignalR connection...');
        await this.hubConnection.start();
        console.log('SignalR connected successfully');
        this.connectionEstablished.next(true);
      } catch (err) {
        console.error('Error starting SignalR connection:', err);
        throw err;
      }
    }
  }
  
  stopConnection(): void {
    if (this.hubConnection.state !== HubConnectionState.Disconnected) {
      this.hubConnection.stop().then(() => console.log('SignalR connection stopped'));
    }
  }
  
  private async ensureConnected(): Promise<void> {
    if (this.hubConnection.state !== HubConnectionState.Connected) {
      await this.startConnection();
    }
  }
  
  async createChat(employerId: string, freelancerId: string, projectId: string): Promise<void> {
    await this.ensureConnected();
    const request = { EmployerId: employerId, FreelancerId: freelancerId, ProjectId: projectId };
    console.log('Creating chat with:', request);
    await this.hubConnection.invoke('CreateChat', request);
  }
  
  async getChatByProjectId(projectId: string): Promise<void> {
    await this.ensureConnected();
    console.log('Getting chat by project ID:', projectId);
    await this.hubConnection.invoke('GetChatByProjectId', projectId);
  }
  
  async sendTextMessage(chatId: string, receiverId: string, text: string): Promise<void> {
    await this.ensureConnected();
    const request = { ChatId: chatId, ReceiverId: receiverId, Text: text };
    console.log('Sending text message:', request);
    await this.hubConnection.invoke('SendTextMessage', request);
  }
  
  async getChatMessages(chatId: string, pageNo: number, pageSize: number): Promise<void> {
    await this.ensureConnected();
    const request = { ChatId: chatId, PageNo: pageNo, PageSize: pageSize };
    console.log('Getting chat messages:', request);
    await this.hubConnection.invoke('GetChatMessages', request);
  }
  
  uploadFile(chatId: string, receiverId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('ChatId', chatId);
    formData.append('ReceiverId', receiverId);
    formData.append('File', file);
    
    console.log('Uploading file to chat:', formData);
    return this.http.post(`${CHAT_SERVICE_API_URL}files`, formData).pipe(
      catchError(error => {
        console.error('Error uploading file:', error);
        return throwError(() => error);
      })
    );
  }
  
  downloadFile(chatId: string, fileId: string): Observable<Blob> {
    const url = `${CHAT_SERVICE_API_URL}files/chat/${chatId}/file/${fileId}`;
    return this.http.get(url, { responseType: 'blob' });
  }
  
  getMessageReceived(): Observable<Message> {
    return this.messageReceived.asObservable();
  }
  
  getChatReceived(): Observable<Chat | null> {
    return this.chatReceived.asObservable();
  }
  
  getMessagesReceived(): Observable<PaginatedResult<Message>> {
    return this.messagesReceived.asObservable();
  }
}