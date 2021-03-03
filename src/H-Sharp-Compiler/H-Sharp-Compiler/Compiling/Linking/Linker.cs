using System;
using System.Collections.Generic;

using HSharp.Analysis;
using HSharp.Analysis.TypeData;
using HSharp.Parsing.AbstractSnyaxTree;
using HSharp.Parsing.AbstractSnyaxTree.Declaration;
using HSharp.Parsing.AbstractSnyaxTree.Directive;
using ValueType = HSharp.Analysis.TypeData.ValueType;

namespace HSharp.Compiling.Linking {
    
    public class Linker {

        private struct LinkerResult {
            public CompileResult result;
            public List<LinkingType> types;
            public LinkerResult(CompileResult result, List<LinkingType> types) {
                this.result = result;
                this.types = types;
            }
            public LinkerResult(bool success) : this(new CompileResult(success), new List<LinkingType>()) { }
            public LinkerResult(List<LinkingType> ltype) : this(new CompileResult(true), ltype ) { }
            public static implicit operator bool(LinkerResult res) => res.result.Success;

        }

        private ASTCompiler m_compiler;
        private Domain m_globalDomain;

        private List<LinkingType> m_exportTypes;

        public List<LinkingType> ExportTypes => this.m_exportTypes;

        public Linker(ASTCompiler compiler, Domain globalDomain) {
            this.m_compiler = compiler;
            this.m_globalDomain = globalDomain;
        }

        public CompileResult Link() {
            var result = this.ExtractDeclaringExportTypes();
            if (!result) {
                return result.result;
            } else {
                this.m_exportTypes = result.types;
            }
            return new CompileResult(true);
        }

        private LinkerResult ExtractDeclaringExportTypes() {
            var trees = this.m_compiler.GetTrees();
            var complete = new List<LinkingType>();
            foreach (var tree in trees) {
                foreach (var node in tree.Root) {
                    var result = this.ExtractDeclaringExportTypesFromNode(node, this.m_globalDomain);
                    if (!result) {
                        return result;
                    } else {
                        complete.AddRange(result.types);
                    }
                }
            }
            return new LinkerResult(complete);
        }

        private LinkerResult ExtractDeclaringExportTypesFromNode(ASTNode node, Domain domain) {
            switch (node) {
                case NamespaceDirectiveNode namespaceDirective:
                    Domain sub = domain.First<NamespaceDomain>(namespaceDirective.Name.FullName);
                    var newTypes = new List<LinkingType>();
                    foreach (var subNode in namespaceDirective.Body.Nodes) {
                        var r = this.ExtractDeclaringExportTypesFromNode(subNode, sub);
                        if (!r) {
                            return r;
                        } else {
                            foreach (var subType in r.types) {
                                subType.FullName = $"{namespaceDirective.Name.FullName}.{subType.FullName}";
                                newTypes.Add(subType);
                            }
                        }
                    }
                    return new LinkerResult(new CompileResult(true), newTypes);
                case ClassDeclNode classDecl:
                    var classType = domain.First<ClassType>(classDecl.LocalClassName);
                    var classLnkType = new LinkingType(classDecl.LocalClassName, 0);
                    List<LinkingType> classDeclTypes = new List<LinkingType>() { classLnkType };
                    foreach (var subClass in classDecl.Classes) {
                        var subClassResult = this.ExtractDeclaringExportTypesFromNode(subClass, classType);
                        if (!subClassResult) {
                            return subClassResult;
                        } else {
                            foreach (var subClassType in subClassResult.types) {
                                subClassType.FullName = $"{classDecl.LocalClassName}+{subClassType.FullName}";
                            }
                        }
                    }
                    foreach(var field in classType.Fields) {
                        classLnkType.FieldPtrs.Add(field.Key, classLnkType.SizeInMemory);
                        if (field.Value is ValueType vType) {
                            classLnkType.SizeInMemory += vType.Size;
                        } else if (field.Value is ReferenceType rType) {
                            classLnkType.SizeInMemory += 8;
                        } else {
                            throw new NotImplementedException();
                        }
                    }
                    return new LinkerResult(classDeclTypes);
                default:
                    return new LinkerResult(true);
            }
        }

    }

}
