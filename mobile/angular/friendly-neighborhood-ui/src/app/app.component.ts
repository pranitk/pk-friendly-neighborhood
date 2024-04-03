import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html', // todo -> replace 
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'friendly-neighborhood-ui';
}
