import {Component, OnInit} from '@angular/core';
import {Track} from "../../../core/models/track";
import {TrackService} from "../../../core/services/tracks.service";
import {MatDialog} from "@angular/material/dialog";
import {TrackDetailsDialogComponent} from "./track-details-dialog/track-details-dialog.component";
import {TrackDataDialogComponent} from "./track-data-dialog/track-data-dialog.component";

@Component({
  selector: 'app-tracks',
  templateUrl: './tracks.component.html',
  styleUrl: './tracks.component.css'
})
export class TracksComponent implements OnInit {
  columns = [
    { key: 'id', label: 'ID' },
    { key: 'name', label: 'Name' }
  ];

  data: Track[] = [];

  actions = [
    {
      label: 'Delete',
      callback: (row: Track) => this.deleteTrack(row.id ?? -1)
    },
    {
      label: 'Update',
      callback: (row: Track) => this.openDataTrackDialog(row)
    }
  ];

  constructor(private trackService: TrackService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadTracks();
  }

  loadTracks(): void {
    this.trackService.getTracks().subscribe(data => {
      console.log(data);
      this.data = data;
    });
  }

  deleteTrack(id: number): void {
    if (confirm('Are you sure you want to delete this track?')) {
      this.trackService.deleteTrack(id).subscribe(() => {
        this.data = this.data.filter(track => track.id !== id);
        alert('Track deleted successfully.');
      }, error => {
        console.error('Error deleting track', error);
        alert('Failed to delete track.');
      });
    }
  }

  onRowClick(track: Track): void {
    const dialogRef = this.dialog.open(TrackDetailsDialogComponent, {
      width: '600px',
      height: '400px',
      data: track
    });
  }

  openDataTrackDialog(track?: Track): void {
    const dialogRef = this.dialog.open(TrackDataDialogComponent, {
      width: '600px',
      data: track
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.loadTracks();
      }
    });
  }
}
