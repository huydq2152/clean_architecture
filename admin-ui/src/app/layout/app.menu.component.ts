import { OnInit } from '@angular/core';
import { Component } from '@angular/core';
import { LayoutService } from './service/app.layout.service';

@Component({
    selector: 'app-menu',
    templateUrl: './app.menu.component.html',
})
export class AppMenuComponent implements OnInit {
    model: any[] = [];

    constructor(public layoutService: LayoutService) {}

    ngOnInit() {
        this.model = [
            {
                label: 'Home',
                items: [
                    {
                        label: 'Trang chủ',
                        icon: 'pi pi-fw pi-home',
                        routerLink: ['/'],
                    },
                ],
            },
            {
                label: 'Content',
                icon: 'pi pi-fw pi-briefcase',
                items: [
                    {
                        label: 'Loại bài viết',
                        icon: 'pi pi-fw pi-pencil',
                        routerLink: ['/content/post-categories'],
                    },
                ],
            },
            {
                label: 'System',
                icon: 'pi pi-fw pi-briefcase',
                items: [
                    {
                        label: 'Người dùng',
                        icon: 'pi pi-fw pi-pencil',
                        routerLink: ['/system/users'],
                    },
                    {
                        label: 'Vai trò',
                        icon: 'pi pi-fw pi-pencil',
                        routerLink: ['/system/roles'],
                    },
                ],
            },
        ];
    }
}
