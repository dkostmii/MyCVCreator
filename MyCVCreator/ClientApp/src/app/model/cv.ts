import { Section } from './section';

export class Container {
  title: string;
  sections: Section[];

  constructor(name: string) {
    this.title = name;
    this.sections = [];
  }

  add(newSection: Section) {
    if (this.sections.some(section => section.id === newSection.id)) {
      throw new Error('Duplicate section being added with id: ' + newSection.id);
    }

    this.sections.push(newSection);
  }

  remove(id: number) {
    if (!this.sections.some(section => section.id === id)) {
      throw new Error(`Section with ${id} was not found.`);
    }

    this.sections = this.sections.filter(s => s.id !== id);
  }

  update(updated: Section) {
    this.sections = this.sections.map(s => {
      if (s.id === updated.id) {
        return updated;
      }

      return s;
    });
  }
}

export interface CV {
  firstName: string,
  lastName: string,
  position: string,
  containers: Container[],
}
