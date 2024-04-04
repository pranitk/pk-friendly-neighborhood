import { AfterViewInit, Component, ElementRef, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {FormsModule} from '@angular/forms';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import { ChatService } from '../services/chat.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ChatResponse, QuestionFormat } from './interfaces';
import { CommonModule } from '@angular/common';
import {MatProgressBarModule} from '@angular/material/progress-bar';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,
    CommonModule, 
    MatCardModule, 
    FormsModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatIconModule, 
    HttpClientModule,
    MatProgressBarModule,
    MatButtonModule],
  templateUrl: './home.component.html', 
  styleUrl: './home.component.scss'
})
export class AppComponent implements OnInit {
  title = 'friendly-neighborhood-ui';
  question = 'How can I help?';
  helpTip = 'Welcome!';
  userInput = '';
  inputPlaceholder = 'I am looking for...';
  responses: ChatResponse[] = [];
  isLoadingResponse = false;

  constructor(private chatService: ChatService) {}

  ngOnInit(): void {
    this.chatService.nextQuestion$.subscribe((ques: QuestionFormat) => { 
      this.question = ques.question;
      this.helpTip = ques.helpTip;
    });

    this.chatService.responses$.subscribe((response) => { 
      this.responses.push(response);
    });

    this.chatService.isLoading$.subscribe((isloading) => {
      this.isLoadingResponse = isloading;
    })
  }

  sendResponse() {
    // this.chatService.sendResponseAndUpdateQuestion(this.userInput); // use this for mock
    if(!this.userInput || this.userInput.trim().length === 0) {
      return;
    }

    this.chatService.sendUserResponse(this.userInput);
    this.userInput = '';
    this.inputPlaceholder = 'Type your response here';
  }

}
