import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ChatService {

  constructor(private httpClient: HttpClient) { }

  getResults(userInput: string): Observable<any> {
    return this.httpClient.get('xyz/abc?query='+userInput);
  }

}