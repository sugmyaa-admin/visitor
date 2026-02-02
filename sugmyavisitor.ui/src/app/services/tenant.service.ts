import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class TenantService {
    private tenant: string;
    private apiBaseUrl: string;
    private isProd = environment.production;

    constructor() {
        const hostname = window.location.hostname;
        const parts = hostname.split('.');
        
        if (this.isProd) {
            this.tenant = parts.length >= 3 ? parts[0] : 'default';
            this.apiBaseUrl = `https://${hostname}/visitor-api/`;
        } else {
            this.tenant = 'default';
            this.apiBaseUrl = `${environment.api_url}visitor-api/`;
        }
    }

    getTenant(): string {
        return this.tenant;
    }

    getApiBaseUrl(): string {
        return this.apiBaseUrl;
    }
}
