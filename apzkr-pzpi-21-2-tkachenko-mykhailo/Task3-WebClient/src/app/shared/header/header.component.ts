import {Component, HostListener} from '@angular/core';
import {AppComponent} from "../../app.component";

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {

  protected readonly AppComponent = AppComponent;

  private lastScrollTop = 0;

  @HostListener('window:scroll', ['$event'])
  onScroll(event: Event): void {
    const currentScroll = window.pageYOffset || document.documentElement.scrollTop;
    const header = document.getElementById('header');

    if (currentScroll > this.lastScrollTop) {
      // Scroll Down
      if (header) {
        header.style.transform = 'translateY(-100%)';
      }
    } else {
      // Scroll Up
      if (header) {
        header.style.transform = 'translateY(0)';
      }
    }

    this.lastScrollTop = currentScroll <= 0 ? 0 : currentScroll;
  }
}
