import {Component, OnInit} from '@angular/core';
import {Project} from '../../../core/interfaces/project/project.interface';
import {FreelancerUser} from "../../../core/interfaces/freelancer/freelancer-user.interface";
import {ActivatedRoute, Router, RouterModule} from "@angular/router";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {UsersService} from "../../../core/services/users/users.service";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzDescriptionsComponent, NzDescriptionsItemComponent, NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {CommonModule, NgIf} from "@angular/common";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzGridModule} from "ng-zorro-antd/grid";
import {ProjectChatComponent} from "./project-chat/project-chat.component";
import {ChatService} from "../../../core/services/chat/chat.service";
import {ProjectStatus} from '../../../core/interfaces/project/lifecycle.interface';

@Component({
  selector: 'app-my-project-info',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzDescriptionsModule,
    NzGridModule,
    RouterModule,
    NzFlexDirective,
    NzButtonComponent,
    NzDescriptionsItemComponent,
    NzDescriptionsComponent,
    NgIf,
    ProjectChatComponent,
  ],
  templateUrl: './my-project-info.component.html',
  styleUrls: ['./my-project-info.component.scss']
})
export class MyProjectInfoComponent implements OnInit {
  project: Project | null = null;
  freelancer: FreelancerUser | null = null;
  loading = false;
  submitting = false;
  
  protected readonly ProjectStatus = ProjectStatus;
  
  constructor(
    private route: ActivatedRoute,
    private projectsService: ProjectsService,
    private usersService: UsersService,
    private chatService: ChatService,
    private message: NzMessageService,
    private router: Router,
  ) {}
  
  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId');
    if (projectId) {
      this.loadProject(projectId);
    }
  }
  
  loadProject(projectId: string): void {
    this.loading = true;
    this.projectsService.getProjectById(projectId).subscribe({
      next: (project: Project) => {
        this.project = project;
        if (project.freelancerId) {
          this.loadFreelancer(project.freelancerId);
        } else {
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.loading = false;
        this.message.error('Failed to load project.');
      }
    });
  }
  
  loadFreelancer(freelancerId: string): void {
    this.usersService.getFreelancerInfo(freelancerId).subscribe({
      next: (freelancer: FreelancerUser) => {
        this.freelancer = freelancer;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading freelancer:', error);
        this.loading = false;
        this.message.error('Failed to load freelancer information.');
      }
    });
  }
  
  getStatusLabel(status: number): string {
    const statuses = [
      'Published',
      'AcceptingApplications',
      'WaitingForWorkStart',
      'InProgress',
      'PendingForReview',
      'Completed',
      'Expired',
      'Cancelled'
    ];
    return statuses[status] || 'Unknown';
  }
  
  canCompleteProject(): boolean {
    if (!this.project || this.project.lifecycle.status === ProjectStatus.Completed) {
      return false;
    }
    const status = this.project.lifecycle.status;
    return (
      status === ProjectStatus.PendingForReview &&
      this.project.lifecycle.acceptanceRequested
    );
  }
  
  async setProjectAcceptanceStatus(isAccepted: boolean): Promise<void> {
    if (!this.project) return;
    this.submitting = true;
    try {
      if (isAccepted) {
        this.project.lifecycle.status = ProjectStatus.Completed;
        this.message.success('Project marked as completed successfully!');
        
        try {
            await this.chatService.setChatInactive(this.project.id);
        } catch (error) {
          console.error('Error deactivating chat:', error);
          this.message.error('Failed to deactivate chat.');
        }
      }
    } catch (error) {
      console.error('Error completing project:', error);
      this.message.error('Failed to complete project.');
    } finally {
      this.submitting = false;
    }
  }
  
  goBack(): void {
    this.router.navigate(['/employer/my-projects']);
  }
}