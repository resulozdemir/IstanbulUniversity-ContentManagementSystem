import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PagesPageComponent } from './pages-page.component';

describe('PagesPageComponent', () => {
  let component: PagesPageComponent;
  let fixture: ComponentFixture<PagesPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PagesPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PagesPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
