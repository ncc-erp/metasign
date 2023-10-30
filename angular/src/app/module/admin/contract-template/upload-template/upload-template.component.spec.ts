import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadTemplateComponent } from './upload-template.component';

describe('UploadTemplateComponent', () => {
  let component: UploadTemplateComponent;
  let fixture: ComponentFixture<UploadTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
