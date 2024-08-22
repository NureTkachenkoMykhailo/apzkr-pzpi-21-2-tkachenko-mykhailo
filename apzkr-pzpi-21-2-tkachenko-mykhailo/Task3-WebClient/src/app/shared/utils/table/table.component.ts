import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-table',
  templateUrl: './table.component.html',
  styleUrls: ['./table.component.css']
})
export class TableComponent {
  @Input() columns: Array<{ key: string, label: string }> = [];
  @Input() data: Array<any> = [];
  @Input() actions: Array<{ label: string, callback: (row: any) => void }> = [];
  @Input() clickableRows: boolean = false;

  @Output() rowClick = new EventEmitter<any>();

  onRowClick(row: any) {
    if (this.clickableRows) {
      this.rowClick.emit(row);
    }
  }

  onActionClick(action: (row: any) => void, row: any, event: Event) {
    event.stopPropagation();
    action(row);
  }

  getPropertyByKeyPath(targetObj: any, keyPath: string) {
    let keys = keyPath.split('.');
    if(keys.length == 0) return undefined;
    keys = keys.reverse();
    let subObject = targetObj;
    while(keys.length) {
      const k = keys.pop();
      if(!subObject.hasOwnProperty(k)) {
        return undefined;
      } else {
        // @ts-ignore
        subObject = subObject[k];
      }
    }
    return subObject;
  }
}
