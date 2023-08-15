import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SignServerComponent } from './sign-server.component';

describe('SignServerComponent', () => {
  let component: SignServerComponent;
  let fixture: ComponentFixture<SignServerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SignServerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SignServerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
