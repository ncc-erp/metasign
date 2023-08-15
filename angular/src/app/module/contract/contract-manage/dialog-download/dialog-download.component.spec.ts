import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogDownloadComponent } from './dialog-download.component';

describe('DialogDownloadComponent', () => {
  let component: DialogDownloadComponent;
  let fixture: ComponentFixture<DialogDownloadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogDownloadComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogDownloadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
