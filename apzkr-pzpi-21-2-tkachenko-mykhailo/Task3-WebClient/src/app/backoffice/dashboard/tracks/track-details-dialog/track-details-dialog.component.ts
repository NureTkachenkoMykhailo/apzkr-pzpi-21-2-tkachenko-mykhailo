import {Component, Inject} from '@angular/core';
import {MAT_DIALOG_DATA} from "@angular/material/dialog";
import {DangerLevel, Track} from "../../../../core/models/track";

@Component({
  selector: 'app-track-details-dialog',
  templateUrl: './track-details-dialog.component.html',
  styleUrls: ['./track-details-dialog.component.css']
})
export class TrackDetailsDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public track: Track) {}

  protected readonly DangerLevel = DangerLevel;
}
