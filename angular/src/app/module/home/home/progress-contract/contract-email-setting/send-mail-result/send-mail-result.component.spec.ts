import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SendMailResultComponent } from './send-mail-result.component';

describe('SendMailResultComponent', () => {
  let component: SendMailResultComponent;
  let fixture: ComponentFixture<SendMailResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SendMailResultComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SendMailResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
