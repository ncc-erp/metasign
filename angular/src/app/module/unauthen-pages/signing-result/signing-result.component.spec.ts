import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SigningResultComponent } from './signing-result.component';

describe('SigningResultComponent', () => {
  let component: SigningResultComponent;
  let fixture: ComponentFixture<SigningResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SigningResultComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SigningResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
