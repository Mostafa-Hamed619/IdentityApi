import { Component, OnInit } from '@angular/core';
import { PlayServicesService } from './play-services.service';

@Component({
  selector: 'app-play',
  templateUrl: './play.component.html',
  styleUrls: ['./play.component.css']
})
export class PlayComponent implements OnInit{

  message :string |undefined
  constructor(private playerServices : PlayServicesService){}
  ngOnInit(): void {
    this.playerServices.getplayers().subscribe({
      next:(response : any)=>{
        this.message = response.value.message
      },
      error:error=>console.log(error)
    })
  }
}
