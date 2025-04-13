import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {NzMessageService} from "ng-zorro-antd/message";
import {Chat} from "../../../core/interfaces/chat/chat.interface";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {ChatsService} from "../../services/chats/chats.service";

@Component({
  selector: 'app-chat-service-tools',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzSpinModule
  ],
  providers: [NzMessageService],
  templateUrl: './chat-service-tools.component.html',
  styleUrl: './chat-service-tools.component.scss'
})
export class ChatServiceToolsComponent implements OnInit {
  chats: Chat[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 10;
  
  constructor(
    private chatsService: ChatsService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadChats();
  }
  
  loadChats(): void {
    this.loading = true;
    this.chatsService.getAllChats(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<Chat>) => {
        this.chats = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Failed to load chats', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadChats();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadChats();
  }
}
