import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { ChatResponse, QuestionFormat, ServerResponse } from '../app/interfaces';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
   responses$: ReplaySubject<ChatResponse> = new ReplaySubject();
   nextQuestion$: ReplaySubject<QuestionFormat> = new ReplaySubject();
   currentAppState: string = 'initial';
   userId: number = 0;
   isLoading$:ReplaySubject<boolean> = new ReplaySubject();

   httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':  'application/json',
    })
  };

   // mocks
   mockQuestionSet: QuestionFormat[] = [
    {
        question: 'Tell me about yourself',
        helpTip: 'I can help you better'
    },
    {
        question: 'Can you share your zip code',
        helpTip: 'I can find similar families around and you can connect with them'
    },
   ];
   currentIndex = 0; // only for mock & demo

  constructor(private httpClient: HttpClient) {
    this.userId = this.getRandomInt(100);
    }

  sendResponseAndUpdateQuestion(response: string) {  

    if(this.currentIndex === 0) {
        this.responses$.next({
            content: 'How can I help you?',
            type: 'text',
            sender: 'service',
        });
    }

    this.responses$.next({
        content: response,
        type: 'text',
        sender: 'user',
    });

    
    // send the next ques from the mock set
    this.mockNextQuestion();
  }

  sendUserResponse(response: string) {  

    if(this.currentIndex === 0) {
        this.responses$.next({
            content: 'How can I help you?',
            type: 'text',
            sender: 'service',
        });
        this.currentIndex++;
    }

    this.responses$.next({
        content: response,
        type: 'text',
        sender: 'user',
    });

    const reqObj: ServerResponse = {
        UserId: `${this.userId}`,
        AppState: this.currentAppState,
        Message: response,
    };
    this.isLoading$.next(true);
    this.httpClient.post('https://usecase4intentiondocker.azurewebsites.net/intention', reqObj, this.httpOptions).subscribe(
        {
            next: (res) => { 
                console.log('response is ...', res)
                const response: any = res;
                this.responses$.next({
                    content: response.message,
                    type: 'text',
                    sender: 'service',
                });
                this.currentAppState = response.appstate;
                this.isLoading$.next(false);
            },
            error: (err) => { 
                console.log('this is NOT working', err);
                this.mockNextQuestion();    
                this.isLoading$.next(false);           
             },
           // complete: () => { console.log('this is working') },
        }
    );
  }

  mockNextQuestion() {
    if(this.currentIndex === this.mockQuestionSet.length) {
        return;
    }
    
    this.responses$.next({
        content: this.mockQuestionSet[this.currentIndex].question,
        type: 'text',
        sender: 'service',
    });
    this.nextQuestion$.next(this.mockQuestionSet[this.currentIndex]);
    this.currentIndex++;
  }

  getRandomInt(max: number) {
    return Math.floor(Math.random() * max);
  }
}
