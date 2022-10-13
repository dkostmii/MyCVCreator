import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCvContainerComponent } from './create-cv-container.component';

describe('CreateCvContainerComponent', () => {
  let component: CreateCvContainerComponent;
  let fixture: ComponentFixture<CreateCvContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateCvContainerComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateCvContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
