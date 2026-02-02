

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class VisitorService {
private readonly getVisitorsPath = "Txn/getVisitorDetails";
private readonly getVisitorsLogin = "Auth";
private readonly getnotification = "Notification/SendWaitTime";
  constructor(private http: HttpClient) {}

  public getVisitors(): Observable<any> {
    
    return this.http.get<any>(this.getVisitorsPath);
  }
  public login(loginId: number, password: string): Observable<any> {
    return this.http.post(`${this.getVisitorsLogin}/login`, { loginId, password });
  }
  sendOtp(mobileNumber: string): Observable<any> {
    return this.http.post(`${this.getVisitorsLogin}/getOtp`, { mobileNumber });
  }

  confirmOtp(mobileNumber: string, otp: string): Observable<any> {
    return this.http.post(`${this.getVisitorsLogin}/verify`, { mobileNumber, otp });
  }
  getUser(): Observable<any> {
    return this.http.get(`${this.getVisitorsLogin}/getEmployeeDetail`);
  }
  resetPassword(username: string, newPassword: string): Observable<any> {
    return this.http.post(`${this.getVisitorsLogin}/reset-password`, { username, newPassword });
  }
  newPassword( oldPassword: string,password: string,oprFlag:string='C'): Observable<any> {
  
    return this.http.post(`${this.getVisitorsLogin}/changePassword`, { oldPassword,oprFlag,password });
  }
  sendNotification(time: number,mobileNumber:string): Observable<any> {
    return this.http.post(`${this.getnotification}/?waitingTime=${time}&mobileNumber=${mobileNumber}`, { time ,mobileNumber});
  }
  forceCheckout(txnId: string, mobileNumber:string, isForceCheckOut: number = 1): Observable<any> {
    const url = `${environment.api_url}Txn/checkout/${mobileNumber}?isForceCheckOut=${isForceCheckOut}&txnId=${txnId}`;
    return this.http.get(url);
  }
  
}
