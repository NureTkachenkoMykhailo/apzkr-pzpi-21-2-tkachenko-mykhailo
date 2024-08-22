import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { BackofficeRoutingModule } from './backoffice-routing.module';
import { BookingsComponent } from './dashboard/bookings/bookings.component';
import { InstructorsComponent } from './dashboard/instructors/instructors.component';
import { LoginComponent } from './login/login.component';
import { ReactiveFormsModule } from "@angular/forms";
import { DashboardComponent } from './dashboard/dashboard.component';
import {SharedModule} from "../shared/shared.module";
import { TracksComponent } from './dashboard/tracks/tracks.component';
import { TrackDetailsDialogComponent } from './dashboard/tracks/track-details-dialog/track-details-dialog.component';
import {MatDialogActions, MatDialogClose, MatDialogContent, MatDialogTitle} from "@angular/material/dialog";
import {MatButton, MatFabButton} from "@angular/material/button";
import { TrackDataDialogComponent } from './dashboard/tracks/track-data-dialog/track-data-dialog.component';
import {MatFormField, MatFormFieldModule} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";
import {MatOption, MatSelect} from "@angular/material/select";
import { TrainingsComponent } from './dashboard/trainings/trainings.component';
import { TrainingDetailsDialogComponent } from './dashboard/trainings/training-details-dialog/training-details-dialog.component';
import { TrainingsDataDialogComponent } from './dashboard/trainings/trainings-data-dialog/trainings-data-dialog.component';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from "@angular/material/datepicker";


@NgModule({
  declarations: [
    BookingsComponent,
    InstructorsComponent,
    LoginComponent,
    DashboardComponent,
    TracksComponent,
    TrackDetailsDialogComponent,
    TrackDataDialogComponent,
    TrainingsComponent,
    TrainingDetailsDialogComponent,
    TrainingsDataDialogComponent
  ],
  imports: [
    MatFormFieldModule,
    CommonModule,
    BackofficeRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    MatDialogClose,
    MatButton,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatFabButton,
    MatFormField,
    MatInput,
    MatSelect,
    MatOption,
    MatDatepickerToggle,
    MatDatepickerInput,
    MatDatepicker
  ]
})
export class BackofficeModule { }
