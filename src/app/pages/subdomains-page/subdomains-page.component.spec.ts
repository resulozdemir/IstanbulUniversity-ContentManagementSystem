import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubdomainsPageComponent } from './subdomains-page.component';

describe('SubdomainsPageComponent', () => {
  let component: SubdomainsPageComponent;
  let fixture: ComponentFixture<SubdomainsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SubdomainsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubdomainsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
