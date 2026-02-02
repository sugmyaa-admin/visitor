import { Component, Input, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss']
})
export class LoadingSpinnerComponent implements OnInit, OnDestroy {
  @Input() class="";
  animationSpeed = 0; // Start fast
  private intervalId: any;

  ngOnInit(): void {
    this.intervalId = setInterval(() => {
      this.animationSpeed += 0.1;
    }, 200);
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalId);
  }
}
