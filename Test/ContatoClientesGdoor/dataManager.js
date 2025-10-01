// ========================================
// MÓDULO DE GERENCIAMENTO DE DADOS JSON
// ========================================

class DataManager {
    constructor() {
        this.companies = [];
        this.filteredCompanies = [];
        this.dataChanged = false;
        this.changeCount = 0;
    }

    // Carrega dados do arquivo JSON
    async loadData() {
        try {
            this.showLoading(true);
            
            const response = await fetch('companies_data.json');
            if (!response.ok) {
                throw new Error('Erro ao carregar dados');
            }
            
            this.companies = await response.json();
            this.filteredCompanies = [...this.companies];
            
            console.log(`✅ Carregadas ${this.companies.length} empresas`);
            return true;
            
        } catch (error) {
            console.error('❌ Erro ao carregar dados:', error);
            alert('Erro ao carregar dados das empresas. Verifique se o arquivo companies_data.json está presente.');
            return false;
        } finally {
            this.showLoading(false);
        }
    }

    // Controla exibição do loading
    showLoading(show) {
        const loading = document.getElementById('loading');
        if (loading) {
            loading.classList.toggle('show', show);
        }
    }

    // Salva dados no localStorage
    saveToLocalStorage() {
        try {
            localStorage.setItem('gdoor_companies', JSON.stringify(this.companies));
            console.log('💾 Dados salvos no localStorage');
        } catch (error) {
            console.error('❌ Erro ao salvar no localStorage:', error);
        }
    }

    // Marca que houve alterações
    markDataChanged() {
        this.dataChanged = true;
        this.changeCount++;
        
        // Dispara evento para atualizar UI
        this.dispatchEvent('dataChanged', {
            changed: this.dataChanged,
            count: this.changeCount
        });
        
        // Auto-salva a cada 10 alterações
        if (this.changeCount >= 10) {
            this.autoSaveJSON();
        }
    }

    // Salva arquivo JSON
    saveJSONFile() {
        try {
            const dataStr = JSON.stringify(this.companies, null, 2);
            const dataBlob = new Blob([dataStr], { type: 'application/json' });
            const url = URL.createObjectURL(dataBlob);
            const link = document.createElement('a');
            
            link.href = url;
            link.download = 'companies_data.json';
            link.style.display = 'none';
            
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
            
            // Reset contador
            this.dataChanged = false;
            this.changeCount = 0;
            
            this.dispatchEvent('dataChanged', {
                changed: this.dataChanged,
                count: this.changeCount
            });
            
            console.log('💾 Arquivo JSON salvo');
            this.dispatchEvent('toast', {
                message: 'Arquivo JSON salvo com sucesso!',
                type: 'success'
            });
            
        } catch (error) {
            console.error('❌ Erro ao salvar arquivo JSON:', error);
            alert('Erro ao salvar arquivo JSON');
        }
    }

    // Auto-save inteligente
    autoSaveJSON() {
        this.saveJSONFile();
        
        const processedCount = this.companies.filter(c => c.status !== 'pending').length;
        this.dispatchEvent('toast', {
            message: `Auto-save: ${processedCount} empresas processadas`,
            type: 'info'
        });
    }

    // Exporta dados com timestamp
    exportData() {
        try {
            const dataStr = JSON.stringify(this.companies, null, 2);
            const dataBlob = new Blob([dataStr], { type: 'application/json' });
            const url = URL.createObjectURL(dataBlob);
            const link = document.createElement('a');
            
            const timestamp = new Date().toISOString().split('T')[0];
            link.href = url;
            link.download = `gdoor_companies_${timestamp}.json`;
            link.click();
            URL.revokeObjectURL(url);
            
            console.log('📤 Dados exportados');
            
        } catch (error) {
            console.error('❌ Erro ao exportar dados:', error);
            alert('Erro ao exportar dados');
        }
    }

    // Atualiza dados de uma empresa
    updateCompany(index, data) {
        if (index >= 0 && index < this.companies.length) {
            Object.assign(this.companies[index], data);
            this.saveToLocalStorage();
            this.markDataChanged();
            
            console.log(`📝 Empresa atualizada: ${this.companies[index].name}`);
            return true;
        }
        return false;
    }

    // Busca empresa por índice
    getCompany(index) {
        return this.companies[index] || null;
    }

    // Busca empresa por CNPJ
    getCompanyByCNPJ(cnpj) {
        return this.companies.find(company => company.cnpj === cnpj);
    }

    // Filtra empresas
    filterCompanies(search = '', city = '', status = '') {
        this.filteredCompanies = this.companies.filter(company => {
            const matchSearch = !search || 
                company.name.toLowerCase().includes(search.toLowerCase()) || 
                company.cnpj.includes(search) ||
                (company.owner && company.owner.toLowerCase().includes(search.toLowerCase()));
                
            const matchCity = !city || company.city === city;
            const matchStatus = !status || company.status === status;
            
            return matchSearch && matchCity && matchStatus;
        });
        
        console.log(`🔍 Filtros aplicados: ${this.filteredCompanies.length} empresas encontradas`);
        return this.filteredCompanies;
    }

    // Obtém lista de cidades únicas
    getCities() {
        return [...new Set(this.companies.map(c => c.city))].sort();
    }

    // Obtém estatísticas
    getStats() {
        return {
            total: this.companies.length,
            pending: this.companies.filter(c => c.status === 'pending').length,
            sent: this.companies.filter(c => c.status === 'sent').length,
            replied: this.companies.filter(c => c.status === 'replied').length
        };
    }

    // Obtém empresas por status
    getCompaniesByStatus(status) {
        return this.companies.filter(c => c.status === status);
    }

    // Verifica se há alterações pendentes
    hasUnsavedChanges() {
        return this.dataChanged;
    }

    // Sistema de eventos
    dispatchEvent(eventType, data) {
        const event = new CustomEvent(`dataManager:${eventType}`, {
            detail: data
        });
        document.dispatchEvent(event);
    }

    // Reseta dados (para desenvolvimento)
    reset() {
        this.companies = [];
        this.filteredCompanies = [];
        this.dataChanged = false;
        this.changeCount = 0;
        
        localStorage.removeItem('gdoor_companies');
        console.log('🔄 Dados resetados');
    }

    // Importa dados de backup
    async importData(file) {
        try {
            const text = await file.text();
            const importedData = JSON.parse(text);
            
            if (Array.isArray(importedData) && importedData.length > 0) {
                this.companies = importedData;
                this.filteredCompanies = [...this.companies];
                this.saveToLocalStorage();
                this.markDataChanged();
                
                console.log(`📥 Importadas ${this.companies.length} empresas`);
                return true;
            } else {
                throw new Error('Formato de arquivo inválido');
            }
        } catch (error) {
            console.error('❌ Erro ao importar dados:', error);
            alert('Erro ao importar dados: ' + error.message);
            return false;
        }
    }

    // Backup automático
    scheduleAutoBackup(intervalMinutes = 30) {
        setInterval(() => {
            if (this.hasUnsavedChanges()) {
                this.autoSaveJSON();
            }
        }, intervalMinutes * 60 * 1000);
        
        console.log(`⏰ Backup automático agendado para cada ${intervalMinutes} minutos`);
    }
}

// Exporta instância única
const dataManager = new DataManager();
export default dataManager;