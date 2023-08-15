import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmailValidComponent } from './email-valid.component';

describe('EmailValidComponent', () => {
  let component: EmailValidComponent;
  let fixture: ComponentFixture<EmailValidComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmailValidComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmailValidComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
