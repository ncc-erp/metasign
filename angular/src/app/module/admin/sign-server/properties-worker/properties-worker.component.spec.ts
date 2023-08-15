import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PropertiesWorkerComponent } from './properties-worker.component';

describe('PropertiesWorkerComponent', () => {
  let component: PropertiesWorkerComponent;
  let fixture: ComponentFixture<PropertiesWorkerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PropertiesWorkerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PropertiesWorkerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
