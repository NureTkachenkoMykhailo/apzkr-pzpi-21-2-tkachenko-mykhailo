import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { SidebarComponent } from './sidebar/sidebar.component';
import {RouterLink} from "@angular/router";
import { TableComponent } from './utils/table/table.component';



@NgModule({
  declarations: [
    HeaderComponent,
    FooterComponent,
    SidebarComponent,
    TableComponent
  ],
    exports: [
        HeaderComponent,
        FooterComponent,
        TableComponent
    ],
  imports: [
    CommonModule,
    RouterLink
  ]
})
export class SharedModule { }
