// ========================================
// GDOOR - APLICAÇÃO PRINCIPAL
// ========================================

import DataManager from './dataManager.js';
import APIConsulter from './apiConsulter.js';
import WhatsAppSender from './whatsappSender.js';

// ========================================
// MÓDULO DE INTERFACE (UI)
// ========================================

class UI {
    constructor() {
        this.toastContainer = null;
        this.setupEventListeners();
    }

    // Configura listeners de eventos
    setupEventListeners() {
        // Eventos do DataManager
        document.addEventListener('dataManager:dataChanged', (e) => {
            this.updateSaveButton(e.detail);
        });

        document.addEventListener('dataManager:toast', (e) => {
            this.showToast(e.detail.message, e.detail.type);
        });

        // Eventos do APIConsulter
        document.addEventListener('apiConsulter:toast', (e) => {
            this.showToast(e.detail.message, e.detail.type);
        });

        document.addEventListener('apiConsulter:updateUI', () => {
            this.renderCompanies();
            this.updateStats();
        });

        // Eventos do WhatsAppSender
        document.addEventListener('whatsappSender:toast', (e) => {
            this.showToast(e.detail.message, e.detail.type);
        });

        document.addEventListener('whatsappSender:updateUI', () => {
            this.renderCompanies();
            this.updateStats();
        });
    }

    // Inicializa interface
    async init() {
        console.log('🚀 Inicializando aplicação GDOOR...');
        
        // Carrega dados
        const loaded = await DataManager.loadData();
        if (!loaded) return;

        // Inicializa componentes da UI
        this.populateCityFilter();
        this.renderCompanies();
        this.updateStats();
        this.updatePreview();
        this.updateSaveButton({ changed: false, count: 0 });

        // Agenda backup automático
        DataManager.scheduleAutoBackup(30);

        console.log('✅ Aplicação inicializada com sucesso!');
    }

    // Popula filtro de cidades
    populateCityFilter() {
        const cities = DataManager.getCities();
        const select = document.getElementById('cityFilter');
        
        if (!select) return;

        select.innerHTML = '<option value="">Todas as cidades</option>';
        cities.forEach(city => {
            select.innerHTML += `<option value="${city}">${city}</option>`;
        });

        console.log(`📍 ${cities.length} cidades carregadas no filtro`);
    }

    // Renderiza lista de empresas
    renderCompanies() {
        const container = document.getElementById('companiesList');
        if (!container) return;

        container.innerHTML = '';

        if (DataManager.filteredCompanies.length === 0) {
            container.innerHTML = this.getNoResultsHTML();
            return;
        }

        DataManager.filteredCompanies.forEach((company) => {
            const realIndex = DataManager.companies.findIndex(c => c.serial === company.serial);
            container.innerHTML += this.getCompanyCardHTML(company, realIndex);
        });

        console.log(`🏢 ${DataManager.filteredCompanies.length} empresas renderizadas`);
    }

    // HTML para quando não há resultados
    getNoResultsHTML() {
        return `
            <div class="text-center py-5">
                <i class="fas fa-search fa-3x text-muted mb-3"></i>
                <h5 class="text-muted">Nenhuma empresa encontrada</h5>
                <p class="text-muted">Tente ajustar os filtros de busca</p>
            </div>
        `;
    }

    // HTML do card da empresa
    getCompanyCardHTML(company, realIndex) {
        const statusClass = `status-${company.status}`;
        const statusIcon = this.getStatusIcon(company.status);
        const phoneFormatted = WhatsAppSender.formatPhone(company.phone);
        const hasValidPhone = phoneFormatted && phoneFormatted !== '0';

        return `
            <div class="card company-card ${statusClass} mb-3">
                <div class="card-body">
                    <div class="row align-items-center">
                        <div class="col-md-5">
                            <h6 class="card-title company-name mb-1">${company.name}</h6>
                            <small class="text-muted d-block">
                                <i class="fas fa-id-card me-1"></i>CNPJ: ${company.cnpj}
                            </small>
                            <small class="text-muted d-block">
                                <i class="fas fa-map-marker-alt me-1"></i>${company.city}, ${company.uf}
                            </small>
                            ${company.owner ? `
                                <small class="text-success d-block">
                                    <i class="fas fa-user me-1"></i><strong>Responsável:</strong> ${company.owner}
                                </small>
                            ` : ''}
                        </div>
                        <div class="col-md-3">
                            <small class="d-block">
                                <i class="fas fa-phone me-1"></i><strong>Telefone:</strong> ${company.phone}
                            </small>
                            ${company.email ? `
                                <small class="d-block">
                                    <i class="fas fa-envelope me-1"></i><strong>Email:</strong> ${company.email}
                                </small>
                            ` : ''}
                            <span class="badge bg-secondary mt-1">
                                <i class="${statusIcon} me-1"></i>${this.getStatusText(company.status)}
                            </span>
                        </div>
                        <div class="col-md-4 text-end">
                            <div class="action-buttons">
                                <button class="btn btn-sm btn-outline-primary" onclick="ui.editCompany(${realIndex})" 
                                        title="Editar informações">
                                    <i class="fas fa-edit"></i>
                                </button>
                                <button class="btn btn-sm btn-outline-info" onclick="ui.consultAPI(${realIndex})" 
                                        title="Consultar dados via API">
                                    <i class="fas fa-sync"></i>
                                </button>
                                <button class="btn btn-sm btn-outline-info" onclick="ui.consultCNPJ('${company.cnpj}')" 
                                        title="Consultar CNPJ no site">
                                    <i class="fas fa-external-link-alt"></i>
                                </button>
                                <button class="btn btn-sm btn-outline-secondary" onclick="ui.testPhone(${realIndex})" 
                                        title="Testar formatação do telefone">
                                    <i class="fas fa-phone-alt"></i>
                                </button>
                                ${hasValidPhone ? `
                                    <button class="btn btn-sm btn-whatsapp" onclick="ui.sendWhatsApp(${realIndex})" 
                                            title="Enviar WhatsApp">
                                        <i class="fab fa-whatsapp"></i>
                                    </button>
                                ` : `
                                    <button class="btn btn-sm btn-secondary" disabled title="Telefone inválido">
                                        <i class="fas fa-phone-slash"></i>
                                    </button>
                                `}
                                <button class="btn btn-sm btn-outline-success" onclick="ui.markAsReplied(${realIndex})" 
                                        title="Marcar como respondeu">
                                    <i class="fas fa-check-double"></i>
                                </button>
                            </div>
                        </div>
                    </div>
                    ${company.notes ? `
                        <div class="row mt-2">
                            <div class="col-12">
                                <div class="alert alert-light py-2 mb-0">
                                    <small><strong>Obs:</strong> ${company.notes}</small>
                                </div>
                            </div>
                        </div>
                    ` : ''}
                </div>
            </div>
        `;
    }

    // Ícones de status
    getStatusIcon(status) {
        const icons = {
            'pending': 'fas fa-clock text-warning',
            'sent': 'fas fa-check text-success',
            'replied': 'fas fa-reply text-info'
        };
        return icons[status] || 'fas fa-question text-muted';
    }

    // Texto do status
    getStatusText(status) {
        const texts = {
            'pending': 'Pendente',
            'sent': 'Enviada',
            'replied': 'Respondeu'
        };
        return texts[status] || status;
    }

    // Atualiza estatísticas
    updateStats() {
        const stats = DataManager.getStats();
        
        document.getElementById('totalCompanies').textContent = stats.total;
        document.getElementById('pendingCount').textContent = stats.pending;
        document.getElementById('sentCount').textContent = stats.sent;
        document.getElementById('repliedCount').textContent = stats.replied;
    }

    // Atualiza preview da mensagem
    updatePreview() {
        const template = document.getElementById('messageTemplate')?.value || '';
        const preview = template
            .replace(/{EMPRESA}/g, 'EXEMPLO EMPRESA LTDA')
            .replace(/{RESPONSAVEL}/g, 'João / Maria');
        
        const previewElement = document.getElementById('messagePreview');
        if (previewElement) {
            previewElement.textContent = preview;
        }
    }

    // Toggle template de mensagem
    toggleMessageTemplate() {
        const section = document.getElementById('messageTemplateSection');
        if (section) {
            const isHidden = section.style.display === 'none';
            section.style.display = isHidden ? 'block' : 'none';
        }
    }

    // Aplica filtros
    applyFilters() {
        const search = document.getElementById('searchFilter')?.value || '';
        const city = document.getElementById('cityFilter')?.value || '';
        const status = document.getElementById('statusFilter')?.value || '';

        DataManager.filterCompanies(search, city, status);
        this.renderCompanies();
    }

    // Abre modal de edição
    editCompany(index) {
        const company = DataManager.getCompany(index);
        if (!company) return;

        document.getElementById('editIndex').value = index;
        document.getElementById('editCompanyName').value = company.name;
        document.getElementById('editCNPJ').value = company.cnpj;
        document.getElementById('editPhone').value = company.phone;
        document.getElementById('editCity').value = company.city;
        document.getElementById('editOwner').value = company.owner || '';
        document.getElementById('editEmail').value = company.email || '';
        document.getElementById('editNotes').value = company.notes || '';

        const modal = new bootstrap.Modal(document.getElementById('editCompanyModal'));
        modal.show();
    }

    // Salva dados da empresa editada
    saveCompany() {
        const index = document.getElementById('editIndex').value;
        const updatedData = {
            name: document.getElementById('editCompanyName').value,
            phone: document.getElementById('editPhone').value,
            city: document.getElementById('editCity').value,
            owner: document.getElementById('editOwner').value,
            email: document.getElementById('editEmail').value,
            notes: document.getElementById('editNotes').value
        };

        DataManager.updateCompany(index, updatedData);

        const modal = bootstrap.Modal.getInstance(document.getElementById('editCompanyModal'));
        modal.hide();
        
        this.applyFilters();
        this.updateStats();
    }

    // Consulta CNPJ no site
    consultCNPJ(cnpj) {
        const cleanCNPJ = cnpj.replace(/\D/g, '');
        const url = `https://cnpj.biz/${cleanCNPJ}`;
        window.open(url, '_blank');
        console.log(`🔗 Abrindo CNPJ.biz para: ${cnpj}`);
    }

    // Consulta todos os CNPJs no site
    consultAllCNPJ() {
        const count = DataManager.companies.length;
        if (confirm(`Isso abrirá ${count} abas do navegador (uma para cada empresa). Deseja continuar?`)) {
            DataManager.companies.forEach((company, index) => {
                setTimeout(() => {
                    this.consultCNPJ(company.cnpj);
                }, index * 1500);
            });
        }
    }

    // Consulta empresa via API
    async consultAPI(index) {
        const success = await APIConsulter.consultCompanyWithFeedback(index);
        if (success) {
            this.renderCompanies();
            this.updateStats();
        }
    }

    // Funções do WhatsApp
    async sendWhatsApp(index) {
        await WhatsAppSender.sendMessage(index);
    }

    markAsReplied(index) {
        WhatsAppSender.markAsReplied(index);
    }

    testPhone(index) {
        WhatsAppSender.testPhone(index);
    }

    // Atualiza botão de salvar
    updateSaveButton(data) {
        const saveBtn = document.querySelector('[onclick*="saveJSONFile"]');
        if (saveBtn) {
            if (data.changed) {
                saveBtn.innerHTML = `<i class="fas fa-save me-1"></i>Salvar JSON (${data.count})`;
                saveBtn.className = 'btn btn-danger me-2';
                saveBtn.classList.add('pulse');
            } else {
                saveBtn.innerHTML = '<i class="fas fa-save me-1"></i>Salvar JSON';
                saveBtn.className = 'btn btn-warning me-2';
                saveBtn.classList.remove('pulse');
            }
        }
    }

    // Sistema de notificações toast
    showToast(message, type = 'info') {
        if (!this.toastContainer) {
            this.createToastContainer();
        }
        
        const toast = document.createElement('div');
        const iconClass = this.getToastIcon(type);
        const bgClass = this.getToastBgClass(type);
        
        toast.className = `alert ${bgClass} alert-dismissible fade show`;
        toast.style.cssText = 'position: relative; min-width: 250px; margin-bottom: 10px;';
        toast.innerHTML = `
            <small>
                <i class="${iconClass} me-1"></i>${message}
            </small>
            <button type="button" class="btn-close btn-close-sm" data-bs-dismiss="alert"></button>
        `;
        
        this.toastContainer.appendChild(toast);
        
        // Remove automaticamente após 3 segundos
        setTimeout(() => {
            if (toast.parentNode) {
                toast.remove();
            }
        }, 3000);
    }

    // Cria container para toasts
    createToastContainer() {
        this.toastContainer = document.createElement('div');
        this.toastContainer.id = 'toastContainer';
        this.toastContainer.style.cssText = 'position: fixed; top: 20px; right: 20px; z-index: 9999; max-width: 350px;';
        document.body.appendChild(this.toastContainer);
    }

    // Ícones para toasts
    getToastIcon(type) {
        const icons = {
            'success': 'fas fa-check-circle',
            'info': 'fas fa-info-circle',
            'warning': 'fas fa-exclamation-triangle',
            'error': 'fas fa-times-circle'
        };
        return icons[type] || 'fas fa-info-circle';
    }

    // Classes de fundo para toasts
    getToastBgClass(type) {
        const classes = {
            'success': 'alert-success',
            'info': 'alert-info',
            'warning': 'alert-warning',
            'error': 'alert-danger'
        };
        return classes[type] || 'alert-info';
    }

    // Utilitários de desenvolvimento
    showPhoneReport() {
        const report = WhatsAppSender.generatePhoneReport();
        console.log(report);
        
        // Cria modal com relatório
        const modal = document.createElement('div');
        modal.innerHTML = `
            <div class="modal fade" tabindex="-1">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">📊 Relatório de Telefones</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                        </div>
                        <div class="modal-body">
                            <pre style="white-space: pre-wrap; font-size: 0.9rem;">${report}</pre>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        const bootstrapModal = new bootstrap.Modal(modal.querySelector('.modal'));
        bootstrapModal.show();
        
        // Remove modal após fechar
        modal.addEventListener('hidden.bs.modal', () => {
            modal.remove();
        });
    }

    // Debug da aplicação
    debug() {
        console.log('🔧 GDOOR Debug Info:');
        console.log('📊 Dados:', DataManager.getStats());
        console.log('📱 Telefones:', WhatsAppSender.getPhoneStats());
        console.log('🌐 API:', APIConsulter.getAPIStats());
        console.log('💾 Alterações pendentes:', DataManager.hasUnsavedChanges());
    }
}

// ========================================
// INICIALIZAÇÃO DA APLICAÇÃO
// ========================================

// Cria instância global da UI
const ui = new UI();

// Expõe no window para compatibilidade com onclick
window.ui = ui;
window.DataManager = DataManager;
window.APIConsulter = APIConsulter;
window.WhatsAppSender = WhatsAppSender;

// Inicializa quando DOM estiver pronto
document.addEventListener('DOMContentLoaded', async () => {
    console.log('🚀 GDOOR - Iniciando aplicação...');
    await ui.init();
});

// Funções globais para compatibilidade com HTML
window.updatePreview = () => ui.updatePreview();
window.toggleMessageTemplate = () => ui.toggleMessageTemplate();

// Atalhos de teclado
document.addEventListener('keydown', (e) => {
    // Ctrl+S para salvar
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        DataManager.saveJSONFile();
    }
    
    // Ctrl+E para exportar
    if (e.ctrlKey && e.key === 'e') {
        e.preventDefault();
        DataManager.exportData();
    }
    
    // F5 para atualizar filtros
    if (e.key === 'F5') {
        e.preventDefault();
        ui.applyFilters();
    }
});

// Aviso ao sair se há alterações não salvas
window.addEventListener('beforeunload', (e) => {
    if (DataManager.hasUnsavedChanges()) {
        e.preventDefault();
        e.returnValue = 'Há alterações não salvas. Deseja realmente sair?';
    }
});

console.log('✅ GDOOR - Módulos carregados com sucesso!');