import {Component, OnInit} from '@angular/core';
import {DataStateChangeEvent, GridDataResult, PageChangeEvent} from "@progress/kendo-angular-grid";
import {CompositeFilterDescriptor, process, SortDescriptor, State} from "@progress/kendo-data-query";
import {Observable} from "rxjs"
import {KendoGridApiService} from "./services/kendo-grid-api.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [KendoGridApiService]

})
export class AppComponent implements OnInit {
  title = 'KendoGridOperations';
  public gridItems: GridDataResult = {
    data: [],
    total: 0
  };
  public pageSize: number = 10;
  public skip: number = 0;
  public sort: SortDescriptor[] = [
    {
      field: 'customerId',
      dir: 'asc'
    }
  ];
  public filter: CompositeFilterDescriptor = {
    logic: 'and',
    filters: [
      {
        field: 'customerId',
        operator: 'contains',
        value: ''
      }
    ]
  };
  public state: State = {
    skip: this.skip,
    take: this.pageSize,
    sort: null! ,
    filter: null!
  };

  constructor(private service: KendoGridApiService) {

  }

  ngOnInit(): void {
    this.loadGridItems();
  }

  public pageChange(event: PageChangeEvent): void {
    this.skip = event.skip;
    this.loadGridItems();
  }

  private loadGridItems(): void {
    this.state.skip = this.skip;
    this.state.take = this.pageSize;

    if(this.sort.length > 0) {
      this.state.sort = this.sort.filter(s => s.dir);
    }

    if(this.filter.filters?.length > 0) {
      this.state.filter = this.filter;
    }


    this.service.getCustomers(this.state).subscribe(
      (data: GridDataResult) => {
        this.gridItems = {
          data: data.data,
          total: data.total
        }
      });
  }

  public handleSortChange(descriptor: SortDescriptor[]): void {
    this.sort = descriptor;
    this.loadGridItems();
  }


  filterChange(filter: CompositeFilterDescriptor) {
    this.filter = filter;
    this.skip = 0;
    this.loadGridItems();
  }

  // public dataStateChange(state: DataStateChangeEvent): void {
  //   this.state = state;
  //   this.gridItems = process(this.riskTypes, this.state);
  // }
}
