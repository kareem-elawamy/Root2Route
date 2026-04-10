import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet], 
  templateUrl: './app.html'
})
export class App { // رجعنا الاسم هنا لـ App عشان main.ts يلاقيه
  title = 'Root2Route';
}