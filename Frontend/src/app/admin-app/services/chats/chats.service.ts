import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {Chat} from "../../../core/interfaces/chat/chat.interface";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ChatsService {

  constructor(private httpClient: HttpClient) { }
  
  getAllChats(pageNo: number, pageSize: number): Observable<PaginatedResult<Chat>> {
    const params = new HttpParams()
      .set('pageNo', pageNo.toString())
      .set('pageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Chat>>(
      `${environment.CHAT_SERVICE_API_URL}chats`,
      { params }
    );
  }
}
