import { Injectable } from '@angular/core';
import { BsModalRef,ModalOptions ,BsModalService} from 'ngx-bootstrap/modal';
import { NotificationComponent } from './components/modals/notification/notification.component';

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  bsModalRef? : BsModalRef
  constructor(private modalServices : BsModalService) { }


  showNotification(isSuccess : boolean ,title : string, message : string){
    const Initialstate : ModalOptions = {
      initialState:{
        isSuccess,
        title,
        message
      }
    };
    this.bsModalRef = this.modalServices.show(NotificationComponent,Initialstate);
  }
}
