import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import {
  Section,
  BulletSectionItem,
  createSectionItem
} from '../model/section';


@Component({
  selector: 'app-create-cv-container',
  templateUrl: './create-cv-container.component.html',
  styleUrls: ['./create-cv-container.component.css']
})
export class CreateCvContainerComponent implements OnInit {

  sections: Section[] = [];

  @Input() title: string = '';

  @Output() newSectionEvent = new EventEmitter<Section>();
  @Output() sectionRemovedEvent = new EventEmitter<number>();
  @Output() sectionUpdatedEvent = new EventEmitter<Section>();

  constructor() { }

  ngOnInit(): void {
  }

  private get sectionNewId(): number {
    return this.sections.reduce(
      (acc, val) => val.id > acc ? val.id : acc, -1) + 1;
  }

  private getItemNewId(s: Section): number {
    return s.items.reduce(
      (acc, val) => val.id > acc ? val.id : acc, -1) + 1;
  }

  addSection(): void {
    const newSection = {
      id: this.sectionNewId,
      title: '',
      items: []
    };

    this.sections.push(newSection);

    this.newSectionEvent.emit(newSection);
  }

  removeSection(id: number) {
    this.sections = this.sections.filter(s => s.id != id);

    this.sectionRemovedEvent.emit(id);
  }

  updateSection(updated: Section) {
    this.sections = this.sections.map(s => {
      if (s.id === updated.id) {
        return updated;
      }

      return s;
    });

    this.sectionUpdatedEvent.emit(updated);
  }

  addTextSectionItem(s: Section): void {
    s.items.push(createSectionItem("text", this.getItemNewId(s)));
    this.updateSection(s);
  }

  addBulletSectionItem(s: Section): void {
    s.items.push(createSectionItem("bullet", this.getItemNewId(s)));
    this.updateSection(s);
  }

  addLabeledSectionItem(s: Section): void {
    s.items.push(createSectionItem("labeled", this.getItemNewId(s)));
    this.updateSection(s);
  }

  removeSectionItem(id: number, s: Section): void {
    s.items = s.items.filter(i => i.id != id);
    this.updateSection(s);
  }

  private getNewListItemId(i: BulletSectionItem): number {
    return i.list.reduce(
      (acc, val) => val.id > acc ? val.id : acc, -1) + 1;
  }

  addListItemBullet(i: BulletSectionItem, s: Section): void {
    i.list.push({ id: this.getNewListItemId(i), text: '' });
    this.updateSection(s);
  }

  removeListItemBullet(id: number, i: BulletSectionItem, s: Section): void {
    i.list = i.list.filter(li => li.id != id);
    this.updateSection(s);
  }
}
