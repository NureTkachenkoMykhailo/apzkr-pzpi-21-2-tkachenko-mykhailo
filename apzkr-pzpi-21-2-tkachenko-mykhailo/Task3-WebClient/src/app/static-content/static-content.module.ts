import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StaticContentRoutingModule } from './static-content-routing.module';
import { AboutComponent } from './about/about.component';
import { ContactComponent } from './contact/contact.component';
import { HomeComponent } from './home/home.component';
import {FormsModule} from "@angular/forms";


@NgModule({
  declarations: [
    AboutComponent,
    ContactComponent,
    HomeComponent
  ],
    imports: [
        CommonModule,
        StaticContentRoutingModule,
        FormsModule
    ]
})
export class StaticContentModule { }
