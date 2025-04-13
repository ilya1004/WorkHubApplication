import {Injectable} from '@angular/core';
import {BehaviorSubject, catchError, Observable, Subject, throwError} from "rxjs";
import {HubConnection, HubConnectionBuilder, HubConnectionState} from "@microsoft/signalr";
import {Chat} from '../../interfaces/chat/chat.interface';
import {Message} from "../../interfaces/chat/message.interface";
import {HttpClient} from "@angular/common/http";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";
import {TokenService} from "../auth/token.service";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: HubConnection;
  private messageReceived = new Subject<Message>();
  private chatReceived = new BehaviorSubject<Chat | null>(null);
  private messagesReceived = new Subject<PaginatedResult<Message>>();
  private connectionEstablished = new Subject<boolean>();
  private isHubInitialized = false;
  
  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(environment.CHAT_SERVICE_HUB_URL, {
        accessTokenFactory: () => this.tokenService.getAccessToken() || '',
        withCredentials: true
      })
      .withAutomaticReconnect()
      .build();
    
    this.initializeHub();
  }
  
  private initializeHub(): void {
    if (this.isHubInitialized) {
      return;
    }
    
    this.hubConnection.on('ReceiveChat', (chat: Chat | null) => {
      this.chatReceived.next(chat);
    });
    
    this.hubConnection.on('ReceiveTextMessage', (message: Message) => {
      this.messageReceived.next(message);
    });
    
    this.hubConnection.on('ReceiveFileMessage', (message: Message) => {
      this.messageReceived.next(message);
    });
    
    this.hubConnection.on('ReceiveChatMessages', (messages: PaginatedResult<Message>) => {
      this.messagesReceived.next(messages);
    });
    
    this.hubConnection.onclose((err) => {
      console.error('SignalR connection closed:', err);
    });
    
    this.isHubInitialized = true;
  }
  
  async startConnection(): Promise<void> {
    if (this.hubConnection.state === HubConnectionState.Disconnected) {
      try {
        await this.hubConnection.start();
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
    await this.hubConnection.invoke('CreateChat', request);
  }
  
  async getChatByProjectId(projectId: string): Promise<void> {
    await this.ensureConnected();
    await this.hubConnection.invoke('GetChatByProjectId', projectId);
  }
  
  async sendTextMessage(chatId: string, receiverId: string, text: string): Promise<void> {
    await this.ensureConnected();
    const request = { ChatId: chatId, ReceiverId: receiverId, Text: text };
    await this.hubConnection.invoke('SendTextMessage', request);
  }
  
  async getChatMessages(chatId: string, pageNo: number, pageSize: number): Promise<void> {
    await this.ensureConnected();
    const request = { ChatId: chatId, PageNo: pageNo, PageSize: pageSize };
    await this.hubConnection.invoke('GetChatMessages', request);
  }
  
  async setChatInactive(projectId: string): Promise<void> {
    if (!this.hubConnection || this.hubConnection.state !== 'Connected') {
      await this.startConnection();
    }
    const request = { chatId: projectId };
    await this.hubConnection.invoke('SetChatInactive', request);
  }
  
  uploadFile(chatId: string, receiverId: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('ChatId', chatId);
    formData.append('ReceiverId', receiverId);
    formData.append('File', file);
    
    return this.http.post(`${environment.CHAT_SERVICE_API_URL}files`, formData).pipe(
      catchError(error => {
        console.error('Error uploading file:', error);
        return throwError(() => error);
      })
    );
  }
  
  downloadFile(chatId: string, fileId: string): Observable<Blob> {
    const url = `${environment.CHAT_SERVICE_API_URL}files/chat/${chatId}/file/${fileId}`;
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