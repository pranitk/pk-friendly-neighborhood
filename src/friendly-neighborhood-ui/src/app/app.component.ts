import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import {MatInputModule} from '@angular/material/input';
import {MatFormFieldModule} from '@angular/material/form-field';
import {FormsModule} from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MatCardModule, FormsModule, MatFormFieldModule, MatInputModule],
  templateUrl: './app.component.html', // todo -> replace 
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'friendly-neighborhood-ui';
}
