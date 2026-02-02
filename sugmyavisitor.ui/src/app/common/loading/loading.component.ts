import { Component,Input, OnInit} from '@angular/core';
import { LoadingService } from 'src/app/common/loading/loading.services';

@Component({
  selector: 'app-loading',
  templateUrl: './loading.component.html',
  styleUrls: ['./loading.component.scss']
})
export class LoadingComponent implements OnInit{
  isLoading:any;
  constructor(private loadingService: LoadingService){
    this.loadingService.loading$.subscribe(res=>{
      this.isLoading=res;
    })
  }
   ngOnInit(): void {
     
   }
  
}
