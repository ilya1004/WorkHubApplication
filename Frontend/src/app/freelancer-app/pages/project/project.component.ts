import { Component } from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {ProjectService} from "../../services/project.service";
import {Project} from "../../interfaces/my-projects/project.interface";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDividerComponent} from "ng-zorro-antd/divider";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {CurrencyPipe, DatePipe, NgForOf, NgIf} from "@angular/common";
import {NzSpinComponent} from "ng-zorro-antd/spin";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzInputDirective} from "ng-zorro-antd/input";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {FormsModule} from "@angular/forms";
import {CHAT_SERVICE_HUB_URL} from "../../../core/constants";
import {ChatComponent} from "./chat/chat.component";

@Component({
  selector: 'app-project',
  imports: [
    NzCardComponent,
    NzDividerComponent,
    NzTagComponent,
    NgIf,
    NzSpinComponent,
    DatePipe,
    CurrencyPipe,
    NzFlexDirective,
    NgForOf,
    NzInputDirective,
    NzButtonComponent,
    FormsModule,
    ChatComponent
  ],
  templateUrl: './project.component.html',
  styleUrl: './project.component.scss'
})
export class ProjectComponent {
  project!: Project;
  projectId!: string;
  messages: string[] = [];
  messageText: string = '';
  private hubConnection!: HubConnection;

  constructor(
      private route: ActivatedRoute,
      private projectService: ProjectService,
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('id')!;
    this.loadProject();
    this.startChatConnection();
  }

  ngOnDestroy(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  loadProject() {
    this.projectService.getProjectById(this.projectId).subscribe({
      next: (res) => this.project = res,
      error: (err) => console.error('Failed to load project', err),
    });
  }

  startChatConnection() {
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(`${CHAT_SERVICE_HUB_URL}`)
        .withAutomaticReconnect()
        .build();

    this.hubConnection.start().then(() => {
      console.log('Connected to chat hub');
      this.getChatMessages();
    }).catch((err: any) => {
      console.error('Error connecting to chat', err)
    });

    this.hubConnection.on('ReceiveTextMessage', (message: string) => {
      this.messages.push(message);
    });

    this.hubConnection.on('ReceiveChatMessages', (chatMessages: any) => {
      this.messages = chatMessages.items.map((m: any) => m.text);
    });
  }

  getChatMessages() {
    this.hubConnection.invoke('GetChatMessages', {
      projectId: this.projectId
    });
  }

  sendMessage() {
    if (!this.messageText) return;

    this.hubConnection.invoke('SendTextMessage', {
      text: this.messageText,
      receiverId: this.project.employerId,
      chatId: this.project.id // Или другой идентификатор чата, если отличается
    });

    this.messageText = '';
  }
}
