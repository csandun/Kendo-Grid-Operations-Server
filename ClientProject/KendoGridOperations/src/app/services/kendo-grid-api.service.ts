import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {GridDataResult} from "@progress/kendo-angular-grid";

@Injectable({
  providedIn: 'root'
})
export class KendoGridApiService {

  constructor(private http: HttpClient) { }

  getCustomers(state: any) {
    return this.http.post<GridDataResult>("https://localhost:7263/filter", state, {
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}
