
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router'; 
import { VisitorService } from 'src/app/services/visitor.service';

@Component({
  selector: 'app-response',
  templateUrl: './response.component.html',
  styleUrls: ['./response.component.scss']
})
export class ResponseComponent implements OnInit {

  response: boolean = true; // Show loading by default
  showTick: boolean = false; // Variable to show success tick
  time: any = 0; // Default value
  mobileNumber: string = ''; // Default value

  constructor(
    private router: Router, 
    private visitorService: VisitorService,
    private activatedRoute: ActivatedRoute // Inject ActivatedRoute to get query params
  ) { }

  ngOnInit(): void {
    // Extract query params from the URL
    this.activatedRoute.queryParams.subscribe(params => {
      this.time = params['waitingTime'] ? +params['waitingTime'] : this.time; // Convert to number
      this.mobileNumber = params['mobileNumber'] ? params['mobileNumber'] : this.mobileNumber;

      // Now call fetchResponse after retrieving the query params
      this.fetchResponse();
    });
  }

  fetchResponse(): void {
    // Send the notification
    this.visitorService.sendNotification(this.time, this.mobileNumber).subscribe({
      next: (data) => {
        // Open the response page in a new tab
        const newTab = window.open('notification', '_blank');

        if (newTab) {
          // Write content to the new tab
          newTab.document.write(`
            <html>
            <head>
              <title>Response Page</title>
              <style>
                .loading-content {
                  text-align: center;
                  margin-top: 80px;
                }
                img {
                  width: 300px;
                }
              </style>
            </head>
            <body>
              <div class="loading-content">
                <img src="../../../assets/images/tic2.gif" alt="Success!" class="success-img">
                <p>Response Successful</p>
              </div>
              <script>
                // Close the tab after 5 seconds
                setTimeout(function() {
                  window.close();
                }, 5000);
              </script>
            </body>
            </html>
          `);
         
        }
        window.close();
      },
      error: (error) => {
        console.error('API error:', error);
      }
    });
  }
}
