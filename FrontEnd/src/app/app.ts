import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
 feature/add-seader
  standalone: true,
  imports: [RouterOutlet],
  template: `<router-outlet />`,
})
export class App {}
 
  imports: [RouterOutlet], 
  templateUrl: './app.html'
})
export class App { // رجعنا الاسم هنا لـ App عشان main.ts يلاقيه
  title = 'Root2Route';
}
  main
