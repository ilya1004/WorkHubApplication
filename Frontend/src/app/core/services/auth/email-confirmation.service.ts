import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class EmailConfirmationService {

  constructor(
    private httpClient: HttpClient
  ) { }

  sendEmailConfirmation(payload: {email: string}) {
    return this.httpClient.post(
      `${environment.IDENTITY_SERVICE_API_URL}auth/resend-email-confirmation`,
      payload
    );
  }

  confirmEmail(payload: {email: string, code: string}) {
    return this.httpClient.post(
      `${environment.IDENTITY_SERVICE_API_URL}auth/confirm-email`,
      payload
    );
  }
}
