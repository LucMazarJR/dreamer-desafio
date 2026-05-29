import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from '../shared/ui/header/header';
import { SideBar } from '../shared/ui/side-bar/side-bar';
import { Footer } from '../shared/ui/footer/footer';
import { LayoutService } from '../shared/layout.service';

@Component({
  selector: 'app-journey',
  imports: [RouterOutlet, Header, SideBar, Footer],
  templateUrl: './journey.html',
})
export class Journey {
  layout = inject(LayoutService);
}
