import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UnAuthenSigningComponent } from './un-authen-signing.component';

describe('UnAuthenSigningComponent', () => {
  let component: UnAuthenSigningComponent;
  let fixture: ComponentFixture<UnAuthenSigningComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UnAuthenSigningComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UnAuthenSigningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
