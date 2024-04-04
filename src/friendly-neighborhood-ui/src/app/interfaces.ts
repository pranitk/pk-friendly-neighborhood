export interface ChatResponse {
    content: string;
    type: 'text'; // add more for links/action etc
    sender: 'user' | 'service';
}

export interface QuestionFormat {
    question: string;
    helpTip: string;
}

export interface ServerResponse {
    UserId: string;
    Message: string;
    AppState: string; // initial, families, diagnosis, resources
}