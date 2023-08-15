import {
  Component,
  Renderer2,
  OnInit
} from '@angular/core';
import { LayoutStoreService } from '@shared/layout/layout-store.service';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit {
  sidebarExpanded: boolean;

  constructor(
    private _layoutStore: LayoutStoreService
  ) { }

  ngOnInit(): void {
    this._layoutStore.sidebarExpanded.subscribe((value) => {
      this.sidebarExpanded = value;
    });
  }
}
