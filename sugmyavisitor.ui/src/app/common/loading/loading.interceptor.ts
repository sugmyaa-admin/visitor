import { Injectable } from '@angular/core'
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http'
// import { LoadingService } from './loading.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, throwError } from 'rxjs'
import { LoadingService } from './loading.services';
import { TenantService } from 'src/app/services/tenant.service';
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    private requests: HttpRequest<any>[] = [];
    urlsToNotUse: Array<string> | undefined;
    constructor(private loaderService: LoadingService, private router: Router, private tenantService: TenantService) {
        this.urlsToNotUse = []
    }
    removeRequest(req: HttpRequest<any>) {
        const i = this.requests.findIndex(function (item, i) {
            return item.url == req.url
        });
        if (i >= 0) {
            this.requests.splice(i, 1);
        }
        if (this.requests.length > 0) {
            this.loaderService.showLoading();
        }
        else {
            this.loaderService.hideLoading();
        }
    }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var url = req.url;
        const isRelative = !req.url.startsWith('http');
        var methodName = url.substring(url.lastIndexOf("/") + 1)
        // if (!this.urlsToNotUse?.includes(methodName)) {
        //     this.requests.push(req);
        //     this.loaderService.showLoading();
        // }
        let tokenInfo = sessionStorage.getItem("authToken");
        const apiUrl = isRelative ? this.tenantService.getApiBaseUrl() + req.url : req.url;
        const authReq = req.clone({
            url: apiUrl,
            setHeaders: {
                Authorization: `Bearer ${tokenInfo}`
            }
        });
        return Observable.create((observer: any) => {
            const subscription = next.handle(authReq)
                .subscribe(
                    event => {
                        if (event instanceof HttpResponse) {
                            this.removeRequest(authReq);
                            observer.next(event);
                        }
                    },
                    err => {
                        this.removeRequest(authReq);
                        observer.error(err);
                        // console.log(err);
                        if (err.status == 401) {
                            this.router.navigate(["login"])
                            //this._cookie.set("previousPath", window.location.pathname)

                        }
                    },
                    () => {
                        this.removeRequest(authReq);
                        observer.complete();
                    });
            // remove request from queue when cancelled
            return () => {
                this.removeRequest(authReq);
                subscription.unsubscribe();
            };
        });
    }
}