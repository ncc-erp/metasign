import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MailDialogComponent } from './mail-dialog.component';

describe('MailDialogComponent', () => {
  let component: MailDialogComponent;
  let fixture: ComponentFixture<MailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
