import { Component, Injector, OnInit } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import {
  Router,
  RouterEvent,
  NavigationEnd,
  PRIMARY_OUTLET,
} from "@angular/router";
import { BehaviorSubject } from "rxjs";
import { filter } from "rxjs/operators";
import { MenuItem } from "@shared/layout/menu-item";

@Component({
  selector: "sidebar-menu",
  templateUrl: "./sidebar-menu.component.html",
  styleUrls: ["./sidebar-menu.component.css"],
})
export class SidebarMenuComponent extends AppComponentBase implements OnInit {
  menuItems: MenuItem[];
  menuItemsMap: { [key: number]: MenuItem } = {};
  activatedMenuItems: MenuItem[] = [];
  routerEvents: BehaviorSubject<RouterEvent> = new BehaviorSubject(undefined);
  homeRoute = "/app/home";

  constructor(injector: Injector, private router: Router) {
    super(injector);
    this.router.events.subscribe(this.routerEvents);
  }

  ngOnInit(): void {
    this.menuItems = this.getMenuItems();
    this.patchMenuItems(this.menuItems);
    this.routerEvents
      .pipe(filter((event) => event instanceof NavigationEnd))
      .subscribe((event) => {
        const currentUrl = event.url !== "/" ? event.url : this.homeRoute;
        const primaryUrlSegmentGroup =
          this.router.parseUrl(currentUrl).root.children[PRIMARY_OUTLET];
        if (primaryUrlSegmentGroup) {
          this.activateMenuItems("/" + primaryUrlSegmentGroup.toString());
        }
      });
  }

  getMenuItems(): MenuItem[] {
    return [
      new MenuItem(this.l(`${this.ecTransform('Home')}`), '/app/home', 'fas fa-home'),
      new MenuItem(this.l(`${this.ecTransform('Admin')}`), '', 'fas fa-circle', 'Admin', [
        new MenuItem(
          this.l(`${this.ecTransform('Role')}`),
          '/app/roles',
          'fas fa-theater-masks',
          'Admin.Role.View'
        ),
        new MenuItem(
          this.l(`${this.ecTransform('Tenant')}`),
          '/app/tenants',
          'fas fa-building',
          'Admin.Tenant.View'
        ),
        new MenuItem(
          this.l(`${this.ecTransform('User')}`),
          '/app/users',
          'fas fa-users',
          'Admin.User.View'
        ),

        new MenuItem(
          this.l(`${this.ecTransform('EmailTemplate')}`),
          '/app/admin/email-templates',
          'fas fa-envelope',
          'Admin.EmailTemplate.View'
        ),

        new MenuItem(
          this.l(`${this.ecTransform('Configuration')}`),
          '/app/admin/configurations',
          'fa-solid fa-sliders',
          'Admin.Configuration.View'
        ),
        new MenuItem(
          'Contract Template',
          '/app/admin/contract-template',
          'fa-solid fa-sliders',
          'Admin.ContractTemplate'
        ),
        new MenuItem(
          this.l(`${this.ecTransform('SignServer')}`),
          '/app/admin/sign-server',
          '',
          'Admin.SignServer'
        ),
      ]),
      new MenuItem(
        this.l(`${this.ecTransform('Signatures')}`),
        '/app/signature',
        'fas fa-signature',
        'Signature.View'
      ),
      new MenuItem(
        this.l(`${this.ecTransform('Contracts')}`),
        '/app/contracts',
        'fa-solid fa-file-contract',
        'Contract.View'
      ),
      new MenuItem(
        this.l(`${this.ecTransform('Contacts')}`),
        '/app/contacts',
        '',
        'Contact.View'
      ),
      new MenuItem(
        this.l(`${this.ecTransform('Templates')}`),
        '/app/templates',
        '',
      )

    ];
  }

  patchMenuItems(items: MenuItem[], parentId?: number): void {
    items.forEach((item: MenuItem, index: number) => {
      item.id = parentId ? Number(parentId + "" + (index + 1)) : index + 1;
      if (parentId) {
        item.parentId = parentId;
      }
      if (parentId || item.children) {
        this.menuItemsMap[item.id] = item;
      }
      if (item.children) {
        this.patchMenuItems(item.children, item.id);
      }
    });
  }

  activateMenuItems(url: string): void {
    this.deactivateMenuItems(this.menuItems);
    this.activatedMenuItems = [];
    const foundedItems = this.findMenuItemsByUrl(url, this.menuItems);
    foundedItems.forEach((item) => {
      this.activateMenuItem(item);
    });
  }

  deactivateMenuItems(items: MenuItem[]): void {
    items.forEach((item: MenuItem) => {
      item.isActive = false;
      item.isCollapsed = true;
      if (item.children) {
        this.deactivateMenuItems(item.children);
      }
    });
  }

  findMenuItemsByUrl(
    url: string,
    items: MenuItem[],
    foundedItems: MenuItem[] = []
  ): MenuItem[] {
    items.forEach((item: MenuItem) => {
      if (item.route === url) {
        foundedItems.push(item);
      } else if (item.children) {
        this.findMenuItemsByUrl(url, item.children, foundedItems);
      }
    });
    return foundedItems;
  }

  activateMenuItem(item: MenuItem): void {
    item.isActive = true;
    if (item.children) {
      item.isCollapsed = false;
    }
    this.activatedMenuItems.push(item);
    if (item.parentId) {
      this.activateMenuItem(this.menuItemsMap[item.parentId]);
    }
  }

  isMenuItemVisible(item: MenuItem): boolean {
    if (!item.permissionName) {
      return true;
    }
    return this.permission.isGranted(item.permissionName);
  }
}
