import { Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-notification',
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.css']
})
export class NotificationComponent {
isSuccess : boolean = true;
message : string = '';
title : string = '';

constructor(public bsModalRef : BsModalRef){}
}
