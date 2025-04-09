export interface Message {
  id: string;
  text?: string;
  fileId?: string;
  createdAt: string;
  senderId: string;
  receiverId: string;
  chatId: string;
  type: MessageType;
}

export enum MessageType {
  Text = 0,
  File = 1
}